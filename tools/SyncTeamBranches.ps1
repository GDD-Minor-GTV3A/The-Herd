param(
	[string]$MainRef = 'origin/main'
)

$ErrorActionPreference = 'Stop'
$mergedBranches = @()
$failedBranches = @()

Write-Host 'Fetching remotes...'
git fetch --all --prune | Out-Null

$remoteBranches = git branch -r --list 'origin/team*' 'origin/Team*' | ForEach-Object { $_.Trim() } | Where-Object { $_ }

if (-not $remoteBranches) {
	Write-Host 'No matching remote branches found.'
	exit 0
}

foreach ($remote in $remoteBranches) {
	$branch = $remote -replace '^origin/', ''
	Write-Host "`n=== Processing $branch ==="

	if (-not (git show-ref --verify --quiet "refs/heads/$branch")) {
		Write-Host "Creating local branch '$branch' from $remote"
		git checkout -b $branch $remote | Write-Host
	} else {
		git checkout $branch | Write-Host
		git reset --hard $remote | Write-Host
	}

	Write-Host "Cleaning up unnecessary files from $branch..."
	git rm --cached .DS_Store
	git rm --cached *.sln
	git rm --cached *.csproj
	git rm --cached *.xml
	git add *
	git commit -m "cleanup branch"
	git push origin $branch | Write-Host "Pushed cleanup changes to remote."

	Write-Host "Merging $MainRef into $branch..."
	git merge $MainRef | Write-Host
	if ($LASTEXITCODE -ne 0) {
		Write-Warning "Merge conflict on $branch. Aborting merge."
		git merge --abort | Out-Null
		$failedBranches += $branch
		continue
	}
    
	$mergedBranches += $branch
	Write-Host "Merge completed on $branch."
    # git push origin $branch | Write-Host "Pushed merged changes to remote."
}

Write-Host "`n=== Summary ==="
Write-Host ("Successful merges ({0}): {1}" -f $mergedBranches.Count, ($mergedBranches -join ', '))
Write-Host ("Failed merges ({0}): {1}" -f $failedBranches.Count, ($failedBranches -join ', '))
