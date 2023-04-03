#/bin/bash
cd pork-backend
docker build -f Dockerfile.controller -t pork-controller .
docker build -f Dockerfile.manager -t pork-manager .
cd ../pork-frontend
docker build -t pork-frontend .