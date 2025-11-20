"""Constants for release module."""
from pathlib import Path

ROOT = Path(__file__).parent.parent
BUILD_DIR = ROOT / "dist"
ZIP_FILE = ROOT / "the-herd.zip"
UNITY = Path("C:/Program Files/Unity/Hub/Editor/6000.0.62f1/Editor/Unity.exe")
UNITY_LOG = ROOT / "unity_build.log"


async def ensure_paths() -> None:
    """Ensure necessary paths exist."""
    BUILD_DIR.mkdir(parents=True, exist_ok=True)

