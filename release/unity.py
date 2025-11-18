"""Build the project using Unity."""
from release.args import UnityArgs
from release.cli import run_command


async def build_project(args: UnityArgs) -> None:
    """Build the project using Unity.exe with the given build profile."""
    await run_command(args.build_command())
