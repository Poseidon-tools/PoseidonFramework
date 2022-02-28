# PoseidonFramework

## Prepare project
Poseidon Framework depends on specific packages, like UniTask, DOTween, MEC, TMP and new Input System. Before adding package, make sure you have installed:
* UniTask
* MEC (with MEC asmdef created in root MEC folder)
* DOTween (with DOTween.Modules asmdef created inside DOTween/Modules folder)
* Newtonsoft JSON Serializer from github package: https://github.com/jilleJr/Newtonsoft.Json-for-Unity
* TMP and new Input system (if not, during framework package installation these packages will be added from dependencies list)
* Make sure you have following asmdefs ready(named in exactly this way as follows): UniTask, Unity.TextMeshPro, DOTWeen.Modules, UniTask.DOTween, Unity.InputSystem, MEC.

## Installation
Add via git package: https://github.com/Poseidon-tools/PoseidonFramework.git?path=/

## Contains
* State Machine 
* Custom DI-framework (Inseminator) [docs: https://github.com/Poseidon-tools/Poseidon-Inseminator]
* Custom messaging system (Postman)
* some various utilities
