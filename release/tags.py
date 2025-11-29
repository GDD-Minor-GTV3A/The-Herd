"""Git tag management."""
from release.cli import run_command
from release.log import logger
from release.version import SemVer

from .args import args


async def get() -> SemVer:
    """Get the current git tag."""
    command = ["git", "describe", "--tags", "--abbrev=0"]
    stdout, _ = await run_command(command)
    version = stdout.decode().strip()

    if not version:
        return SemVer(0, 0, 0)

    logger.info("Current tag: %s", version)
    return SemVer.from_str(version)


async def create() -> None:
    """Create a git tag for the release."""
    logger.info("Creating git tag %s.", args.tag)

    command = ["git", "tag", str(args.tag)]
    await run_command(command)


async def push() -> None:
    """Push the created tag to origin."""
    logger.info("Pushing git tag %s to origin.", args.tag)

    command = ["git", "push", "origin", str(args.tag)]
    await run_command(command)
    logger.info("Tag %s created and pushed successfully.", args.tag)
