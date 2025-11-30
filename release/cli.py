"""CLI utilities for the release module."""
import asyncio

from release.args import args
from release.log import log_command, logger


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


async def test_gh_cli() -> None:
    """Test if GH CLI is installed and authenticated."""
    command = ["gh", "auth", "status"]
    try:
        _, _ = await run_command(command)
    except RuntimeError as e:
        msg = "GH CLI is not installed or authenticated. Install and authenticate it: https://cli.github.com/"
        raise RuntimeError(msg) from e
    logger.debug("GH CLI is installed and authenticated.")
