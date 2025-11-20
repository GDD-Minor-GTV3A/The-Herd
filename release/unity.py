"""Build the project using Unity."""
from release.args import UnityArgs
from release.cli import run_command
from release.log import logger


async def build_project(args: UnityArgs) -> None:
    """Build the project using Unity.exe with the given build profile."""
    logger.info("Building project with Unity. This may take a while...")
    await run_command(args.build_command())
