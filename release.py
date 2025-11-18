#usr/bin/env python3
"""Create a new build, release and tags.

Tags current commit and publishing it to GH releases.

Skips the debug information in dist/*DoNotShip
Builds a zip out of dist/* as an artifact to publish to GH releases.

see documentation for arguments: https://cli.github.com/manual/gh_release_create
additionally --tag, --log, --dist-dir and --zip-file are supported to override the default settings.
use --dry-run to test the script without actually creating a release.

prepend with `no` to set boolean flags to false.
    example: `--no-latest`

Requirements:
    Make sure you have python 3.14 or higher installed. (Lower version may work, but are not tested)
    Make sure you have the GH CLI installed and authenticated: https://cli.github.com/

USAGE:
    python release.py [options]
    python release.py --help

Full release process example, using default values. Minimal required flags:
    python ./release.py --compile --create-tag --upload-release

Only compile:
    python ./release.py --compile

Only create and push tag:
    python ./release.py --create-tag

Only upload release (make sure game is already built and tag is created):
    python ./release.py --upload-release
"""

import asyncio
from logging import INFO

from release import tags
from release.args import args, set_defaults, unity_args
from release.cli import test_gh_cli
from release.log import logger
from release.unity import build_project
from release.upload import upload_release
from release.zip import zip_dist


async def setup_environment() -> None:
    """Set up the environment by running necessary checks concurrently."""
    await asyncio.gather(
        set_defaults(),
        test_gh_cli(),
    )

async def main() -> None:
    """Entry point."""
    logger.setLevel(args.log.upper() or INFO)
    await setup_environment()
    if args.compile:
        await build_project(unity_args)
    if args.create_tag:
        await tags.create()
        await tags.push()
    if args.upload_release:
        await zip_dist(args.dist_dir, args.zip_file)
        await upload_release()

if __name__ == "__main__":
    asyncio.run(main())
