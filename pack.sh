#!/bin/bash

zip -r Labs.zip ./Labs -x "*/bin/*" "*/obj/*" "*/.nuget/*" "*/dist/*"
