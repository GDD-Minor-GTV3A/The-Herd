#usr/bin/env python3
"""Create a new build, release and tags.

Create a new build of the project using Unity, create a new git tag and push it,
and upload a new release to GitHub with the build as an artifact.
If marked as latest, the release will be marked as the latest release on GitHub.

Requirements:
    Make sure you have python 3.14 or higher installed. (Lower version may not work)
    Make sure you have the GH CLI installed and authenticated: https://cli.github.com/

USAGE:
    python release.py [options]
    python release.py --help

Examples:
Examples are shown to be as minimal as possible, only including the necessary flags.

Full release process (all steps):
    python ./release.py --unity --create-tag --upload-release

Only unity (Defaults to compiling and building the project.):
    python ./release.py --unity

Only create and push tag:
    python ./release.py --create-tag

Only upload release (make sure game is already built and tag is created):
    python ./release.py --upload-release

Notes:
Skips the debug information in dist/*DoNotShip
Builds a zip out of dist/* as an artifact to publish to GH releases.

see documentation for arguments: https://cli.github.com/manual/gh_release_create
additionally --tag, --log, --dist-dir and --zip-file are supported to override the default settings.
use --dry-run to test the script without actually creating a release.

prepend with `no` to set boolean flags to false.
    example: `--no-latest`

"""

import asyncio

from release import tags
from release.args import args, set_defaults, unity_args
from release.cli import test_gh_cli
from release.log import logger
from release.paths import check_paths, ensure_paths
from release.unity import run_unity
from release.upload import upload_release
from release.zip import zip_dist


async def setup_environment() -> None:
    """Set up the environment by running necessary checks concurrently."""
    await asyncio.gather(
        set_defaults(),
        test_gh_cli(),
        ensure_paths(),
        check_paths()
    )

async def main() -> None:
    """Entry point."""
    logger.setLevel(args.log.upper())
    await setup_environment()
    if args.unity:
        await run_unity(unity_args)
    if args.create_tag:
        await tags.create()
        await tags.push()
    if args.upload_release:
        await zip_dist(args.dist_dir, args.zip_file)
        await upload_release()

if __name__ == "__main__":
    asyncio.run(main())
