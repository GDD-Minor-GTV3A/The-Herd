"""Create a new release, tagging the current commit and publishing it to GH releases.

Skips the debug information in dist/*DoNotShip
Builds a zip out of dist/* as an artifact to publish to GH releases.

see documentation for arguments: https://cli.github.com/manual/gh_release_create
additionally --tag, --dist-dir and --zip-file are supported to override the default paths.

prepend with `no` to set boolean flags to false.
    example: `--no-latest`

Make sure you have the GH CLI installed and authenticated:
    https://cli.github.com/

USAGE:
    python release.py [options]
    python release.py --help
"""

from __future__ import annotations

import asyncio
import zipfile
from argparse import ArgumentParser, Namespace
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
    verify_tag: bool = False

    tag: SemVer | None = None
    dist_dir: Path = BUILD_DIR
    zip_file: Path = ZIP_FILE

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
                    logger.debug("Unused setting type/value: %s=%r", i, val)


parser = ArgumentParser(description="Create a new GH release.")
for arg, typ in Args.__annotations__.items():
    match typ:
        case bool():
            parser.add_argument(f"--{arg.replace('_', '-')}", action="store_true")
            parser.add_argument(f"--no-{arg.replace('_', '-')}", action="store_false", dest=arg)
        case str():
            parser.add_argument(f"--{arg.replace('_', '-')}", type=str, default=getattr(Args, arg))
        case Path():
            parser.add_argument(f"--{arg.replace('_', '-')}", type=Path, default=getattr(Args, arg))
        case _:
            logger.debug("Unknown argument type: %s", typ)
args: Args = parser.parse_args(namespace=Args())


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


async def zip_dist() -> None:
    """Zip the dist folder into a single zip file for GH releases."""
    logger.info("Zipping files from %s to %s", BUILD_DIR, ZIP_FILE)
    with zipfile.ZipFile(ZIP_FILE, "w") as zf:
        for file in BUILD_DIR.glob("**/*"):
            if "DoNotShip" in file.name:
                continue
            zf.write(file, file.relative_to(BUILD_DIR))
            logger.debug("Added file to zip: %s", file)


async def get_tag() -> SemVer:
    """Get the current git tag."""
    proc = await asyncio.create_subprocess_exec(
        *["git", "describe", "--tags", "--abbrev=0"],
        stdout=asyncio.subprocess.PIPE,
        stderr=asyncio.subprocess.PIPE,
    )
    stdout, stderr = await proc.communicate()
    if proc.returncode != 0:
        msg = f"Failed to get git tag: {stderr.decode()}"
        raise RuntimeError(msg)
    logger.info("Current tag: %s", stdout.decode().strip())
    return SemVer.from_str(stdout.decode().strip())


async def create_release() -> None:
    """Create a GH release."""
    # Build argument list to avoid shell quoting/globbing issues
    command: list[str] = f"gh release create {args.tag} {args.zip_file}".split()
    command += args.generate_setting_flags()

    logger.debug("Creating release with command: %s", command)
    logger.info("Uploading release %s.", args.tag)
    proc = await asyncio.create_subprocess_exec(
        *command,
        stdout=asyncio.subprocess.PIPE,
        stderr=asyncio.subprocess.PIPE,
    )
    stdout, stderr = await proc.communicate()
    if proc.returncode != 0:
        msg = f"Failed to create release: {stderr.decode()}"
        raise RuntimeError(msg)
    logger.info("Release created successfully: %s", stdout.decode())


async def set_defaults() -> None:
    """Set default values for settings if not provided."""
    tag = await get_tag()
    tag = tag.bump_minor()

    # Set default title and notes if not provided
    args.tag = args.tag or tag
    args.title = args.title or f"Release {tag}"
    args.notes = args.notes or f"Automated release of version {tag}."
    logger.info("Successfully set up context for release.")


async def test_gh_cli() -> None:
    """Test if GH CLI is installed and authenticated."""
    proc = await asyncio.create_subprocess_exec(
        *["gh", "auth", "status"],
        stdout=asyncio.subprocess.PIPE,
        stderr=asyncio.subprocess.PIPE,
    )
    _, stderr = await proc.communicate()
    if proc.returncode != 0:
        logger.debug("GH CLI auth status stderr: %s", stderr.decode())
        msg = "GH CLI is not installed or authenticated. Install and authenticate it: https://cli.github.com/"
        raise RuntimeError(msg)
    logger.debug("GH CLI is installed and authenticated.")


async def main() -> None:
    """Entry point."""
    await asyncio.gather(
        zip_dist(),
        set_defaults(),
    )
    await create_release()


if __name__ == "__main__":
    asyncio.run(main())
