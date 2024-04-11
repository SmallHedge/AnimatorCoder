AnimatorCoder by Small Hedge Games 08/04/2024

Note: Crossfade is not affected when playing the RESET animation through script or through the ONPARAMETER inspector!

Setup:
1. Fill in Animator States and Animator Parameters in AnimatorValues
2. Use the SHG.AnimatorCoder namespace
3. Inherit AnimatorCoder in the desired script
4. Implement DefaultAnimation(int layer) method in desired script
5. Run Initalize() in the Start() method

Public Methods:

void Initialize() - Sets up the AnimatorCoder 
bool Play(AnimationData data, int layer) - Attempts to play an animation and returns its success
Animations GetCurrentAnimation(int layer) - Return current animation on layer
void SetLocked(bool lockLayer, int layer) - Set a layer to be locked
bool IsLocked(int layer) - Is the layer locked?
void SetBool(Parameters id, bool value) - Sets a parameter
bool GetBool(Parameters id) - Returns a parameter

Tutorial: https://youtu.be/9tvDtS1vYuM
