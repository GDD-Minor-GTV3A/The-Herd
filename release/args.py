"""Argument parsing for release script."""
from __future__ import annotations

from argparse import ArgumentDefaultsHelpFormatter, ArgumentParser
from argparse import Namespace as BaseNamespace
from pathlib import Path
from typing import TYPE_CHECKING

from release.log import logger
from release.paths import BUILD_DIR, BUILD_PROFILE, EXE, ROOT, UNITY, ZIP_FILE
from release.version import SemVer

if TYPE_CHECKING:
    from collections.abc import Generator

parser = ArgumentParser(description="Create a new GH release.", formatter_class=ArgumentDefaultsHelpFormatter)

class Namespace(BaseNamespace):
    """Base class for argument namespaces."""

    def __init_subclass__(cls) -> None:
        """Automatically add arguments based on annotations."""
        for arg, typ in cls.__annotations__.items():
            default = getattr(cls, arg)
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


class Args(Namespace):
    """Additional settings for release script."""

    # Control flags, on what to do during the release process.
    compile: bool = False # If true, first compile the project using Unity.exe.
    create_tag: bool = False # If true, create a git tag for the release
    upload_release: bool = False # If true, upload the release to GH releases

    # Release settings.
    tag: SemVer | str = "v0.0.0" # Custom tag to use for the release
    dist_dir: Path = BUILD_DIR # Directory to zip for the release
    zip_file: Path = ZIP_FILE # Zip file to create for the release
    log: str = "INFO" # Log level, e.g. DEBUG, INFO, WARNING, ERROR
    dry_run: bool = False # If true, do not create the release


class GithubArgs(Namespace):
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

    def generate_setting_flags(self) -> Generator[str]:
        """Generate command line flags from settings as a list of tokens."""
        for i in GithubArgs.__annotations__:
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

class UnityArgs(Namespace):
    """Arguments for the unity exe."""

    # Unity executable settings. matches https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html
    unityPath: Path = UNITY
    activeBuildProfile: Path = BUILD_PROFILE
    build: Path = EXE
    buildTarget: str = "standalonewindows64"
    projectPath: Path = ROOT
    logFile: str = "-"
    skipMissingProjectId: bool = True
    skipMissingUpid: bool = True
    batchmode: bool = True
    quit: bool = True

    def build_command(self) -> list[str]:
        """Generate the command line to run unity with the given arguments."""
        command = [str(self.unityPath)]
        for arg, val in self.__dict__.items():
            if arg not in self.__annotations__:
                continue
            flag = f"-{arg[0].lower()}{arg[1:]}"
            match val:
                case True:
                    command.append(flag)
                case False | None:
                    continue
                case Path() | str():
                    command.append(flag)
                    command.append(str(val))
                case _:
                    logger.warning("Unused unity argument type or value: %s=%r", arg, val)
        return command


args: Args = parser.parse_args(namespace=Args())
github_args: GithubArgs = parser.parse_args(namespace=GithubArgs())
unity_args: UnityArgs = parser.parse_args(namespace=UnityArgs())


async def set_defaults() -> None:
    """Set default values for settings if not provided."""
    from release import tags
    tag = await tags.get()
    tag = tag.bump_minor()

    # Set default title and notes if not provided
    args.tag = args.tag if args.tag != "v0.0.0" else tag
    github_args.title = github_args.title or f"Release {tag}"
    github_args.notes = github_args.notes or f"Automated release of version {tag}."
    logger.info("Successfully set up context for release.")
