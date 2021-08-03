unity-frosted-glass [![License](https://img.shields.io/badge/License-MIT-lightgrey.svg?style=flat)](http://mit-license.org) [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/andyduboc/5usd)
===============

This code reproduce a frosted glass effect in Unity (as seen in DOOM 2k16). It uses a CommandBuffer attached to the main camera to render the scene in screen-space rendertextures. A grayscale mask is then used to sample between these rendertextures in order to apply the desired blur.

 ![screenshot](~Screenshots/screen0.gif)

* Updated by Nothke to support normal displacement

 ![screenshot](~Screenshots/displacement.gif)

Limitations
===============

* Editor view renders the effect as long as the "Post Processings" option is on
* The project is updated to Unity 2019.4 version, but the FrostedGlass core scripts & shaders are likely compatible to a few previous versions

Further Reading
===============

 - [Extending Unity 5 rendering pipeline: Command Buffers](https://blogs.unity3d.com/2015/02/06/extending-unity-5-rendering-pipeline-command-buffers/)
 - [DOOM (2016) - Graphics Study](http://www.adriancourreges.com/blog/2016/09/09/doom-2016-graphics-study/)

License
===============

MIT, see [LICENSE](LICENSE) for details.
