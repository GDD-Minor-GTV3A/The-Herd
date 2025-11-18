"""Constants for release module."""
from pathlib import Path

ROOT = Path(__file__).parent.parent
BUILD_DIR = ROOT / "dist"
ZIP_FILE = ROOT / "the-herd.zip"
UNITY = Path("C:/Program Files/Unity/Hub/Editor/6000.0.62f1/Editor/Unity.exe")
BUILD_PROFILE = Path(ROOT/"Assets/Settings/Build Profiles/WindowsProfile.asset")
EXE = Path(ROOT/"dist/The Herd.exe")
