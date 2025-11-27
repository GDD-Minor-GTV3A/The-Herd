"""Argument parsing for release script."""
from __future__ import annotations

from argparse import ArgumentDefaultsHelpFormatter, ArgumentParser, Namespace
from enum import Enum
from pathlib import Path
from typing import TYPE_CHECKING, Generic, TypeVar

from release.log import logger
from release.paths import BUILD_DIR, ROOT, UNITY, UNITY_LOG, ZIP_FILE
from release.version import SemVer

if TYPE_CHECKING:
    from collections.abc import Callable, Generator

parser = ArgumentParser(description="Create a new GH release.", formatter_class=ArgumentDefaultsHelpFormatter)
NULL_SEMVER = SemVer(0,0,0)

class ArgumentActions(Enum):
    """Possible argument actions."""

    STORE = "store"
    STORE_TRUE = "store_true"
    STORE_FALSE = "store_false"
    STORE_BOOL = "store_bool" # Custom action to store bools
    STORE_CONST = "store_const"
    APPEND = "append"
    APPEND_CONST = "append_const"
    EXTEND = "extend"
    COUNT = "count"
    HELP = "help"
    VERSION = "version"

T = TypeVar("T")

class Argument(Generic[T]):
    """Helper to define arguments with argparse."""

    def __init__(
        self,
        type_: Callable[..., T] = str,
        action: ArgumentActions = ArgumentActions.STORE,
        default: object | None = None,
        help: str = "",  # noqa: A002
    ) -> None:
        """Initialize argument."""
        self.action = action
        self.type = type_
        self.default = default
        self.help = help

        if self.action is ArgumentActions.STORE_BOOL:
            self.type = bool # This requires `pyright: ignore` in __get__ :(

    def __set_name__(self, owner: type, name: str) -> None:
        """Set the name of the attribute to the name of the descriptor."""
        self.setup_parser_argument(name)
        self.name = name
        self.private_name = f"__{name}"

    def __get__(self, obj: object, obj_type: object) -> T:
        """Get the value of the attribute."""
        return getattr(obj, self.private_name)

    def __set__(self, obj: object, value: T) -> None:
        """Set the value of the attribute."""
        setattr(obj, self.private_name, value)

    def setup_parser_argument(self, name: str) -> None:
        """Set up the argument in the parser."""
        help_ = f"{self.help} - {self.type.__name__}" if self.help else f"{self.type.__name__}"
        if self.action is ArgumentActions.STORE_BOOL:
            parser.add_argument(
                f"--{name.replace('_', '-')}",
                action="store_true",
                dest=name,
                help=help_,
            )
            parser.add_argument(
                f"--no-{name.replace('_', '-')}",
                action="store_false",
                dest=name,
                help="",
            )
        else:
            parser.add_argument(
                f"--{name.replace('_', '-')}",
                type=self.type,
                action=self.action.value,
                default=self.default,
                help=help_,
            )


class Args(Namespace):
    """Additional settings for release script."""

    # Control flags, on what to do during the release process.
    unity = Argument(
        action=ArgumentActions.STORE_BOOL,
        default=False,
        help="Run Unity.exe with the given arguments UnityArgs.",
    )
    create_tag = Argument(
        action=ArgumentActions.STORE_BOOL,
        default=False,
        help="Create a git tag for the release.",
    )
    upload_release = Argument(
        action=ArgumentActions.STORE_BOOL,
        default=False,
        help="Upload the release to GH releases.",
    )
    dry_run = Argument(
        action=ArgumentActions.STORE_BOOL,
        default=False,
        help="If true, only show what would have happened, without actually running anything.",
    )

    # Release settings.
    tag = Argument[str](
        str,
        default=NULL_SEMVER,
        help="The git tag to create for the release.",
    )
    dist_dir = Argument(
        Path,
        default=BUILD_DIR,
        help="Directory containing build artifacts to include in the release.",
    )
    zip_file = Argument(
        Path,
        default=ZIP_FILE,
        help="Path to the zip file to create for the release.",
    )
    log = Argument(
        default="INFO",
        help="The log LEVEL to use for the release script.",
    )



class GithubArgs(Namespace):
    """Command line context."""

    # GitHub CLI release settings. matches https://cli.github.com/manual/gh_release_create.
    discussion_category = Argument(
        default=None,
        help="Discussion category for the release notes.",
    )
    draft = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Mark the release as a draft.",
    )
    fail_on_no_commits = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Fail if there are no new commits since the last release.",
    )
    generate_notes = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Generate release notes automatically.",
    )
    latest = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Mark the release as the latest release. Mark as false to explicitly NOT set as latest",
    )
    notes = Argument[str | None](
        help="Additional notes for the release.",
    )
    notes_file = Argument[Path | None](
        Path,
        default=None,
        help="Read release notes from file (use '-' to read from standard input).",
    )
    notes_from_tag = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Fetch notes from the tag annotation or message of commit associated with tag.",
    )
    notes_start_tag = Argument[str | None](
        help="Tag to use as the starting point for generating release notes",
    )
    prerelease = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Mark the release as a prerelease.",
    )
    target = Argument(
        default="main",
        help="Target branch or full commit SHA",
    )
    title = Argument[str | None](
        help="Title for the release.",
    )
    verify_tag = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Verify the tag exists on remote. Abort if it does not.",
    )

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
    unityPath = Argument(
        Path,
        default=UNITY,
        help="Path to the Unity executable.",
    )
    buildTarget: Argument = Argument(
        default="standalonewindows64",
        help="Build target platform.",
    )
    projectPath: Argument = Argument(
        Path,
        default=ROOT,
        help="Path to the Unity project.",
    )
    logFile: Argument = Argument(
        Path,
        default=UNITY_LOG,
        help="Path to the Unity log file.",
    )
    skipMissingProjectId: Argument = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Skip missing project ID.",
    )
    skipMissingUpid: Argument = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Skip missing UPID.",
    )
    batchmode: Argument = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Run Unity in batch mode.",
    )
    quit: Argument = Argument(
        action=ArgumentActions.STORE_BOOL,
        help="Quit Unity after executing commands.",
    )
    executeMethod: Argument = Argument(
        default="CommandLineBuild.BuildGame",
        help="Method to execute after launching Unity.",
    )

    def build_command(self) -> list[str]:
        """Generate the command line to run unity with the given arguments."""
        command = [str(self.unityPath)]
        for arg in self.__annotations__:
            if arg == "unityPath":
                continue

            val = getattr(self, arg)
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
    args.tag = args.tag if args.tag != str(NULL_SEMVER) else str(tag)
    github_args.title = github_args.title or f"Release {tag}"
    github_args.notes = github_args.notes or f"Automated release of version {tag}."
    logger.info("Successfully set up context for release.")
