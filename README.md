# Usage
```
$ UpdateVersions [file] [options]

file:

The Path to file containing assembly version (AssemblyInfo.cs or .rc)

options:

/fileversion      file version in format major.minor.build.revision 
                  (it could be a template for version change like +0.+0.+0.+1)
/productversion   product version in format major.minor.build.revision
/copyright        copyright
```

# Example
```
$ UpdateVersions AssemblyInfo.cs /fileversion:+0.+0.+0.+1 /productversion:+0.+0.+0.+1 /copyright:"(c) 2019 My Company"
```
