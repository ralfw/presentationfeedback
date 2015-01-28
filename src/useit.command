#!/bin/bash
rm -rd app.events
mono coapp.console.exe conf1 "conference #1" "conf1 - conference number one.txt"
mono afapp.console.exe overview conf1 -now:2015-01-23T9:55:00