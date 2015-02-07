rmdir app.events /s /q

coapp.console.exe upload -id:conf1 -title:"conference #1" -filename:"conf1 - conference number one.txt"
REM coapp.console.exe upload -id:conf1 -title:"conference #2" -filename:"conf2.txt"

afapp.console.exe overview -id:conf1 -now:"2015-01-23T09:55:00"

afapp.console.exe vote -id:s1 -s:Red -c:"Booring!" -e:peter@yahoo.com
afapp.console.exe vote -id:s1 -s:Green -c:"Great" -e:susan@gmail.com
afapp.console.exe vote -id:s1 -s:Yellow -e:dave@yahoo.com
afapp.console.exe vote -id:s1 -s:Green -c:"I learned a lot" -e:hugo@mail.com
afapp.console.exe vote -id:s1 -s:Red -c:"Didn't make sense to me" -e:olga@gmail.com
afapp.console.exe vote -id:s1 -s:Green -e:finn@hotmail.com

afapp.console.exe vote -id:s2 -s:Green -e:peter@yahoo.com
afapp.console.exe vote -id:s2 -s:Red -c:"Why???" -e:susan@gmail.com
afapp.console.exe vote -id:s2 -s:Red -e:dave@yahoo.com
afapp.console.exe vote -id:s2 -s:Yellow -c:"Not interesting!" -e:hugo@mail.com
afapp.console.exe vote -id:s2 -s:Green -c:"I loooved it" -e:olga@gmail.com
afapp.console.exe vote -id:s2 -s:Red -e:finn@hotmail.com