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

    def __eq__(self, other: object) -> bool:
        """Check equality."""
        if not isinstance(other, SemVer):
            return NotImplemented
        return (self.major, self.minor, self.patch) == (other.major, other.minor, other.patch)

    def __lt__(self, other: SemVer) -> bool:
        """Less than comparison."""
        return (self.major, self.minor, self.patch) < (other.major, other.minor, other.patch)

    def __le__(self, other: SemVer) -> bool:
        """Less than or equal comparison."""
        return (self.major, self.minor, self.patch) <= (other.major, other.minor, other.patch)

    def __ne__(self, other: object) -> bool:
        """Check inequality."""
        return not self.__eq__(other)

    def __gt__(self, other: SemVer) -> bool:
        """Greater than comparison."""
        return not self.__le__(other)

    def __ge__(self, other: SemVer) -> bool:
        """Greater than or equal comparison."""
        return not self.__lt__(other)
