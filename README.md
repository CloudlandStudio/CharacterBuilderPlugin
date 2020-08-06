# CharacterBuilderPlugin
 unity character customization tools
 
 # 教學1  tutorial 1 
 [完整教學](https://youtu.be/u8Qak7jCYSk)  
<a href="http://www.youtube.com/watch?feature=player_embedded&v=u8Qak7jCYSk
" target="_blank"><img src="http://img.youtube.com/vi/u8Qak7jCYSk/0.jpg" 
alt="完整教學(with my poor English subtitle)" width="240" height="180" border="10" /></a>  

 # 教學2  tutorial 2
 [使用情境](https://youtu.be/SnMDkYy-Rqk)  
<a href="http://www.youtube.com/watch?feature=player_embedded&v=SnMDkYy-Rqk
" target="_blank"><img src="http://img.youtube.com/vi/u8Qak7jCYSk/0.jpg" 
alt="使用情境" width="240" height="180" border="10" /></a>  

 # 說明  
 ## CharacterBuilder  
 用於客製化角色的主要組件，儲存了骨架Transform和骨架名稱的映射，SkinnedMeshRenderer內的bones會使用骨架的名稱來找到相對應的骨架  
 This component use Dictionary to store bone Transform and bone name, so SkinnedMeshRenderer.bones can map to correct bone.  
 ## CharacterBuilder.CustomPart  
 這個類是角色的部件插槽  
 this class is a character part slot.  
 ## AutoLoadPart  
 簡單的在Update中自動更新CharacterBuilder的部件，使用[ExecuteInEditMode]  
 this component use Update and ExecuteInEditMode, and CharacterBuilder can auto update its part in Editor mode.  
 ## CustomPartData  
 角色身上部件的資料，儲存了載入部件要實例化的物件清單，例如；一組的前後髮、一組的皮膚和服裝、單獨的飾品  
 this is part data of character, it contains a list with part objects, it could be a bundle of hair, skin with is shirt, or just aa accessory.  
 ## SkinnedBaker  
 因為SkinnedMeshRenderer的bones是用Transform來儲存，如果將A角色身上的部件移動到B角色，將會因為參照仍然在A身上而無法讓穿著部件的B角色的動畫正常影響部件，所以SkinnedBaker使用string[]來儲存對應的骨架名稱，點擊bakeIt之後就會自動將SkinnedMeshRenderer的資訊拷貝到SkinnedBaker了
 because SkinnedMeshRenderer serializes bones with Transform, so you can't move a part from character A to B, its bone map still on character A.  
 SkinnedBaker ueses string[] to serialize bones name, you can click 'bake it' to serialize them.
 ## TransformBaker  
 有些部件，例如；臉，是放置在角色的頭骨架內(以Unity Chan的角色來說)，所以必須記錄部件的父物件和本身的Transform資訊，因此設計了這個組件來儲存這些資訊，點擊bakeIt就可以拷貝父物件的資訊到組件上了  
 some part (like Unity Chan's face) was put inside skeleton, so we must record parent and its Transform data, you can use this component to serialize these data by click 'bake it'  
 ## AnimatorPreview  
 用來在編輯器模式下預覽角色的動畫，可以不用進入播放模式直接檢查角色的動畫，這個組件的功能仍然不完善，有時候會無法正常使用  
 AnimatorPreview can help you play animation of animator in editor mode, this component not very sensitive, try it more than once to activate it.(still don't know why maybe it will be fix in someday)
