# Unity-Dependency-Checker
Select a fold, click "GetAllDependency".It will show you all dependencies in this fold.  
Select foldA, click "SetSearchPath". Select foldB, click "CheckAssets".It will show you what file is not referenced by foldA.  
Select foldA, click "SetSearchPath". Select an assetA, click "CheckAssets".It will show you what file references assetA in foldA.   

Copy foldA, get foldB. foldB has some references in foldA.
Window/CopyDependency  
Drag foldB in "Target Files".  
Drag foldA in "Change Dependencies From"  
Drag foldB in "To".  
You change all foldB's dependencies in foldA to foldB.  
