#usr/bin/env python3
"""Create a new release, tagging the current commit and publishing it to GH releases.

Skips the debug information in dist/*DoNotShip
Builds a zip out of dist/* as an artifact to publish to GH releases.

see documentation for arguments: https://cli.github.com/manual/gh_release_create
additionally --tag, --log, --dist-dir and --zip-file are supported to override the default settings.
use --dry-run to test the script without actually creating a release.

prepend with `no` to set boolean flags to false.
    example: `--no-latest`

Requirements:
    Make sure you have python 3.14 or higher installed. (Lower version may work, but are not tested)
    Make sure you have the GH CLI installed and authenticated: https://cli.github.com/

USAGE:
    python release.py [options]
    python release.py --help
"""

from __future__ import annotations

import asyncio
import zipfile
from argparse import ArgumentDefaultsHelpFormatter, ArgumentParser, Namespace
from dataclasses import dataclass
from logging import INFO, StreamHandler, getLogger
from pathlib import Path
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from collections.abc import Generator

ROOT = Path(__file__).parent
BUILD_DIR = ROOT / "dist"
ZIP_FILE = ROOT / "the-herd.zip"

logger = getLogger(__name__)
logger.addHandler(StreamHandler())
logger.setLevel(INFO)


GH_ARGS = [
    "discussion_category",
    "draft",
    "fail_on_no_commits",
    "generate_notes",
    "latest",
    "notes",
    "notes_file",
    "notes_from_tag",
    "notes_start_tag",
    "prerelease",
    "target",
    "title",
    "verify_tag",
]


class Args(Namespace):
    """Command line context."""

    # GitHub CLI release settings. matches https://cli.github.com/manual/gh_release_create.
    discussion_category: str | None = None
    draft: bool = True
    fail_on_no_commits: bool = True
    generate_notes: bool = True
    latest: bool = True
    notes: str | None = None
    notes_file: Path | None = None
    notes_from_tag: bool = False
    notes_start_tag: str | None = None
    prerelease: bool = False
    target: str = "main"
    title: str | None = None
    verify_tag: bool = True

    # Additional settings
    tag: SemVer | str = "v0.0.0" # Custom tag to use for the release
    dist_dir: Path = BUILD_DIR # Directory to zip for the release
    zip_file: Path = ZIP_FILE # Zip file to create for the release
    log: str = "INFO" # Log level, e.g. DEBUG, INFO, WARNING, ERROR
    dry_run: bool = False # If true, do not create the release

    def generate_setting_flags(self) -> Generator[str]:
        """Generate command line flags from settings as a list of tokens."""
        for i in GH_ARGS:
            val = getattr(self, i)
            match val:
                # true  -> add flag
                # false -> skip (we support --no- flags via parser)
                case True:
                    yield f"--{i.replace('_', '-')}"
                case str() | Path():
                    yield f"--{i.replace('_', '-')}"
                    yield str(val)
                case _:
                    logger.warning("Unused setting type or value: %s=%r", i, val)


@dataclass
class SemVer:
    """Semantic Versioning representation."""

    major: int
    minor: int
    patch: int

    @staticmethod
    def from_str(version: str) -> SemVer:
        """Create a SemVer from a string."""
        major, minor, patch = map(int, version.lstrip("v").split("."))
        return SemVer(major, minor, patch)

    def __str__(self) -> str:
        """Convert to string."""
        return f"v{self.major}.{self.minor}.{self.patch}"

    def bump_minor(self) -> SemVer:
        """Bump the minor version."""
        return SemVer(self.major, self.minor + 1, 0)


parser = ArgumentParser(description="Create a new GH release.", formatter_class=ArgumentDefaultsHelpFormatter)
for arg, typ in Args.__annotations__.items():
    default = getattr(Args, arg)
    if "str" in typ:
        parser.add_argument(f"--{arg.replace('_', '-')}", type=str, default=default, help="(str)")
    elif "bool" in typ:
        parser.add_argument(f"--{arg.replace('_', '-')}", action="store_true", dest=arg, help="(bool)")
        parser.add_argument(f"--no-{arg.replace('_', '-')}", action="store_false", dest=arg, help="(bool)")
    elif "Path" in typ:
        parser.add_argument(f"--{arg.replace('_', '-')}", type=Path, default=default, help="(Path)")
    elif "SemVer" in typ:
        parser.add_argument(f"--{arg.replace('_', '-')}", type=SemVer.from_str, default=default, help="(SemVer)")
    else:
        logger.warning("Unknown argument type: %s", typ)
args: Args = parser.parse_args(namespace=Args())


async def run_command(command: list[str]) -> tuple[bytes, bytes]:
    """Run a command asynchronously."""
    if args.dry_run:
        logger.info("Dry run: would run command: %s", command)
        return b"", b""

    proc = await asyncio.create_subprocess_exec(
        *command,
        stdout=asyncio.subprocess.PIPE,
        stderr=asyncio.subprocess.PIPE,
    )
    stdout, stderr = await proc.communicate()
    log_command(command, stdout, stderr)
    if proc.returncode != 0:
        msg = f"Failed to run {command}: {stderr.decode()}"
        raise RuntimeError(msg)
    return stdout, stderr

def log_command(command: list[str], stdout: bytes, stderr: bytes) -> None:
    """Log a command's output."""
    cmd_msg = " ".join(command)
    stdout_msg = stdout.decode().strip()
    stderr_msg = stderr.decode().strip()
    logger.debug("%s: %s %s", cmd_msg, stdout_msg, stderr_msg)

async def zip_dist() -> None:
    """Zip the dist folder into a single zip file for GH releases."""
    logger.info("Zipping files from %s to %s", BUILD_DIR, ZIP_FILE)
    with zipfile.ZipFile(ZIP_FILE, "w") as zf:
        for file in BUILD_DIR.glob("**/*"):
            if "DoNotShip" in str(file):
                logger.debug("Skipping DoNotShip file: %s", file)
                continue
            zf.write(file, file.relative_to(BUILD_DIR))
            logger.debug("Added file to zip: %s", file)


async def get_tag() -> SemVer:
    """Get the current git tag."""
    command = ["git", "describe", "--tags", "--abbrev=0"]
    stdout, _ = await run_command(command)
    version = stdout.decode().strip()

    if not version:
        return SemVer(0, 0, 0)

    logger.info("Current tag: %s", version)
    return SemVer.from_str(version)


async def create_tag() -> None:
    """Create a git tag for the release."""
    logger.info("Creating git tag %s.", args.tag)

    command = ["git", "tag", str(args.tag)]
    await run_command(command)

async def push_tag() -> None:
    """Push the created tag to origin."""
    logger.info("Pushing git tag %s to origin.", args.tag)

    command = ["git", "push", "origin", str(args.tag)]
    await run_command(command)
    logger.info("Tag %s created and pushed successfully.", args.tag)

async def create_release() -> None:
    """Create a GH release."""
    # Build argument list to avoid shell quoting/globbing issues
    logger.info("Uploading release %s.", args.tag)

    command: list[str] = f"gh release create {args.tag} {args.zip_file}".split()
    command += args.generate_setting_flags()

    stdout, _ = await run_command(command)
    logger.info("Release created successfully: %s", stdout.decode())


async def set_defaults() -> None:
    """Set default values for settings if not provided."""
    tag = await get_tag()
    tag = tag.bump_minor()

    # Set default title and notes if not provided
    args.tag = args.tag if args.tag != "v0.0.0" else tag
    args.title = args.title or f"Release {tag}"
    args.notes = args.notes or f"Automated release of version {tag}."
    logger.info("Successfully set up context for release.")


async def test_gh_cli() -> None:
    """Test if GH CLI is installed and authenticated."""
    command = ["gh", "auth", "status"]
    try:
        _, _ = await run_command(command)
    except RuntimeError as e:
        msg = "GH CLI is not installed or authenticated. Install and authenticate it: https://cli.github.com/"
        raise RuntimeError(msg) from e
    logger.debug("GH CLI is installed and authenticated.")


async def main() -> None:
    """Entry point."""
    logger.setLevel(args.log.upper() or INFO)
    await asyncio.gather(
        zip_dist(),
        set_defaults(),
        test_gh_cli(),
    )
    await create_tag()
    await push_tag()
    await create_release()


if __name__ == "__main__":
    asyncio.run(main())
