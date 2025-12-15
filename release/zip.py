"""Zip the dist folder for GH releases."""
import zipfile
from pathlib import Path

from release.args import args
from release.log import logger


def _add_file(zf: zipfile.ZipFile, file: Path, build: Path) -> None:
    """Add a file to the zip, skipping DoNotShip files."""
    if "DoNotShip" in str(file):
        logger.debug("Skipping DoNotShip file: %s", file)
        return
    if args.dry_run:
        logger.info("Dry run: would add file to zip: %s", file)
        return
    zf.write(file, file.relative_to(build))
    logger.debug("Added file to zip: %s", file)

async def zip_dist(build: Path, zip_file: Path) -> None:
    """Zip the dist folder into a single zip file for GH releases."""
    exists = zip_file.exists()
    clone = zip_file.with_suffix(".zip.bak")
    zip_file.copy(clone) if exists else None

    logger.info("Zipping files from %s to %s", build, zip_file)
    with zipfile.ZipFile(zip_file, "w", compression = zipfile.ZIP_DEFLATED) as zf:
        for file in build.glob("**/*"):
            _add_file(zf, file, build)

    # Restore state on dry run
    if args.dry_run:
        if exists:
            zip_file.unlink()
            clone.rename(zip_file)
        else:
            zip_file.unlink()
        logger.info("Dry run: restored previous zip state.")
        return
