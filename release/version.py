from __future__ import annotations

from dataclasses import dataclass


@dataclass
class SemVer:
    """Semantic Versioning representation."""

    major: int
    minor: int
    patch: int

    @staticmethod
    def from_str(version: str) -> SemVer:
        """Create a SemVer from a string."""
        major, minor, patch = map(int, version.lstrip("v").split("."))
        return SemVer(major, minor, patch)

    def __str__(self) -> str:
        """Convert to string."""
        return f"v{self.major}.{self.minor}.{self.patch}"

    def bump_minor(self) -> SemVer:
        """Bump the minor version."""
        return SemVer(self.major, self.minor + 1, 0)
