#!/bin/bash
rm -rd app.events
mono coapp.console.exe upload -id:conf1 -title:"conference #1" -filename:"conf1 - conference number one.txt"
mono afapp.console.exe overview -id:conf1 -now:2015-01-23T9:55:00
mono afapp.console.exe vote -id:s1 -s:Yellow -c:"ok session" -e:peter@yahoo.com