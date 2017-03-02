# Pitch
## A minimal document generator with layout support written in .NET core.

Pitch makes it easy to create layout templates for any file format. Simply add a line at the top of your file specify the path to the layout file and Pitch inject the files contents into the layout, while preserving the file structure of the directory.

### Layout files
Any file can be a layout file. Simple put @layout-outlet anywhere in the file another files contents will be replaced at that location.

### Partial files
Any file can be a partial file, even layout files. A partial file's contents will be injected into a layout file if its first line declares the path to a layout. As long as it matches the pattern layout="../path/to/layout"  then it is considered correct. This is a flexible pattern so you can wrap it inside HTML comments, CSS comments, or anything block you want. It just must be the first line in the file.

### CLI Options 
There are four options that can be set with CLI flags.
- --dir (defaults to current working directory)
- --pattern (defaults to match any file)
- --outdir (defaults to pitch-output)
- --diagnostics (defaults to 'warnings')

#### Examples
> pitch --dir src/docs --pattern *.html
This looks in the src/docs folder for all files ending in .html. It would then create a folder in the current working directory called pitch-output containing the exact directory structure of src/docs where directories were found containing .html files.

> pitch --dir src/docs --pattern *.html --outdir build/docs
Same as above, but the output is saved in a docs folder inside the build folder found in your current working directory. If the build folder does not exist it will be created.

