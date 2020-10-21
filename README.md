# PFind
Command line utility to search for patterns in files

One example (that I've needed it for)
Somewhere in an old project, you look for a string property called "Data" that contains delimited values
So the this value will probably be splitted in some way to use the different values
Usually this can be done by loading the project, finding the property, look for usages of that property, and then find how the values are read
But in some cases this can not be done... right now I'm trying to "decode" an old SilverLight-project, that I can't even load in Visual Studio, and the original authors are not available...
So, using PFind, I can at least get some help

Executing the following
"PFind data split *.cs /R"
I get a report of every CS-file that on the same line contains both "data" and "split" (none case-sensitive), and what line-number the hits are found on

I find that useful :-)
