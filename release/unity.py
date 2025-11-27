"""Build the project using Unity."""
from release.args import UnityArgs
from release.cli import run_command
from release.log import logger


async def run_unity(args: UnityArgs) -> None:
    """Run Unity.exe with the given arguments."""
    logger.info("Running Unity. This may take a while...")
    await run_command(args.build_command())
