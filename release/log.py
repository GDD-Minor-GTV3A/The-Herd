"""Logging utilities for release process."""
from logging import INFO, StreamHandler, getLogger

logger = getLogger(__name__)
logger.addHandler(StreamHandler())
logger.setLevel(INFO)


def log_command(command: list[str], stdout: bytes, stderr: bytes) -> None:
    """Log a command's output."""
    cmd_msg = " ".join(command)
    stdout_msg = stdout.decode().strip()
    stderr_msg = stderr.decode().strip()
    logger.debug("%s: %s %s", cmd_msg, stdout_msg, stderr_msg)
