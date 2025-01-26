@echo off
setlocal

REM Set your variables
set "S3_BUCKET=fk-unity-snippets-555069934636"
set "CLOUDFRONT_DISTRIBUTION_ID=ENVE7BC766TL4"
set "BUILD_DIR=build"

REM Sync the local build directory with the S3 bucket
echo Syncing %BUILD_DIR% with s3://%S3_BUCKET%...

aws s3 cp build/ s3://%S3_BUCKET%/ --recursive --content-encoding br --exclude "*" --include "*.br"
aws s3 cp build/ s3://%S3_BUCKET%/ --recursive --exclude "*.br"

REM Invalidate the CloudFront cache
echo Creating CloudFront invalidation...
aws cloudfront create-invalidation --distribution-id %CLOUDFRONT_DISTRIBUTION_ID% --paths "/*" > nul 2>&1

echo Deployment complete.

endlocal
pause