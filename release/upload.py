"""Upload a release to GitHub."""
from __future__ import annotations

from release.args import args, github_args
from release.cli import run_command
from release.log import logger


async def upload_release() -> None:
    """Create a GH release."""
    # Build argument list to avoid shell quoting/globbing issues
    logger.info("Uploading release %s.", args.tag)

    command: list[str] = f"gh release create {args.tag} {args.zip_file}".split()
    command += github_args.generate_setting_flags()

    stdout, _ = await run_command(command)
    logger.info("Release created successfully: %s", stdout.decode())
