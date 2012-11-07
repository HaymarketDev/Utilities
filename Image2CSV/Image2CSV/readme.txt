This program takes a directory of images and scales and converts them to a csv with the intensity values at each pixel.

Parameters: 
IMAGE_DIRECTORY - Directory that holds the images.  It doesn't do a recursive search, but can be easily changed.
FILE_NAME - CSV file to write the data to.  It will prompt to overwrite if it exists.
REQUESTED_DIMENSIONS - What size (in pixels) to scale the image to.
DELIMETER - Defaults to comma, but can be changed if you want another delimiter (tab, semicolon, etc.)

Could be useful for machine learning applications like the Kaggle digit recognizer competition:
http://www.kaggle.com/c/digit-recognizer/
