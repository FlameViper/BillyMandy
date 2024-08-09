using NAudio.Wave;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioLoader : MonoBehaviour {
 
    [SerializeField] private Button showEnemiesOnDeathButton;
    [SerializeField] private Button showEnemiesOnHitButton;
    [SerializeField] private Button showProjectilesButton;
    [SerializeField] private Button showBGMButton;
    [SerializeField] private Button importAllButton;
    [SerializeField] private Button resetAllButton;
    [SerializeField] private GameObject fileButtonPrefab; // Prefab for the file buttons
    [SerializeField] private GameObject fieldButtonPrefab; // Prefab for the field buttons includes test audio button
    [SerializeField] private GameObject addAudioButtonPrefab; // Prefab for the field buttons includes test audio button
    [SerializeField] private Transform fileListContainer; // Container to hold the list of buttons
    [SerializeField] Sprite EnemyOnDeathIcon;
    public Vector2 startPosition = new Vector2(-645, 315); // Starting position for the first button
    public int spacingX = 215; // Spacing between buttons
    public int spacingY = 90; // Spacing between buttons
    public int spacingYFields = 120; // Spacing between buttons
    public int maxRowSize = 7; // Maximum number of buttons per row

    private AudioSource audioSource;
    private string savedAudioPath;
    private List<string> audioFiles;
    private List<string> audioFields;
    GalleryCategory currentGalleryCategory;


    private Queue<FileProcessData> fileQueue = new Queue<FileProcessData>();
    private bool isProcessing = false;

    private void Start() {
        audioSource = gameObject.GetOrAdd<AudioSource>();   
        ClearAudioContainer();
 
        savedAudioPath = Path.Combine(Application.persistentDataPath, "SavedAudio");
        Directory.CreateDirectory(savedAudioPath);
        foreach (var galleryCategory in SoundManager.Instance.audioGalleryEntries.GetCategories()) {
            if (galleryCategory != null) {
                string categoryPath = Path.Combine(savedAudioPath, galleryCategory.name);
                Directory.CreateDirectory(categoryPath);

                foreach (var field in galleryCategory.GetAudioClipFields()) {
                    string fieldPath = Path.Combine(categoryPath, field.Name);
                    Directory.CreateDirectory(fieldPath);
                }
            }
        }

        showProjectilesButton.onClick.AddListener(() => DisplayCategoryFields(SoundManager.Instance.audioGalleryEntries.ProjectilesCategory));
        showEnemiesOnDeathButton.onClick.AddListener(() => DisplayCategoryFields(SoundManager.Instance.audioGalleryEntries.EnemyOnDeathCategory));
        showEnemiesOnHitButton.onClick.AddListener(() => DisplayCategoryFields(SoundManager.Instance.audioGalleryEntries.EnemyOnHitCategory));
        showBGMButton.onClick.AddListener(() => DisplayCategoryFields(SoundManager.Instance.audioGalleryEntries.BGMCategory));
        importAllButton.onClick.AddListener(() => StartCoroutine(ImportAllFromFolder()));
        resetAllButton.onClick.AddListener(() => OnResetDefaultsClicked());
    
    }
    public void ClearAudioContainer() {
        foreach (Transform child in fileListContainer) {
            Destroy(child.gameObject); 
        }
    }
    private void DisplayCategoryFields(GalleryCategory galleryCategory) {

        string path = Path.Combine(savedAudioPath, galleryCategory.name.ToString());
        Directory.CreateDirectory(path);
        var audioGalleryEntries = galleryCategory.GetAudioClipFields();        
        ClearAudioContainer();
        int currentButtonIndex = 0;
        foreach (var field in audioGalleryEntries) {
            string fieldName = Path.GetFileNameWithoutExtension(field.Name);
            currentGalleryCategory = galleryCategory;
            CreateFieldButton(fieldName,path, currentButtonIndex);
            currentButtonIndex++;
        }
    }
    private void CreateFieldButton(string fieldName,string categoryPath, int index) {
        string path = Path.Combine(categoryPath, fieldName);
        Directory.CreateDirectory(path);
        GameObject buttonObj = Instantiate(fieldButtonPrefab, fileListContainer);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = fieldName;
        Button testButton = buttonObj.transform.GetChild(2).GetComponent<Button>();
        Button resetButton = buttonObj.transform.GetChild(3).GetComponent<Button>();
   
        button.onClick.AddListener(() => OnFieldButtonClicked(fieldName, categoryPath));
        testButton.onClick.AddListener(() => OnTestAudioButtonClicked(fieldName));
        resetButton.onClick.AddListener(() => ResetFieldToDefault(fieldName));
        var audioGalleryIcons = currentGalleryCategory.GetIconFields();
       
        foreach (var iconField in audioGalleryIcons) {
            string iconFieldName = Path.GetFileNameWithoutExtension(iconField.Name);
            string associatedField = fieldName + "Icon";

            if (iconFieldName == associatedField) {

                Image buttonIcon = buttonObj.transform.GetChild(0).GetComponent<Image>();
                Sprite sprite= iconField.GetValue(currentGalleryCategory) as Sprite;
                if (sprite != null) {

                    buttonIcon.sprite = sprite;
                }

            }
        }

        int row = index / maxRowSize;
        int col = index % maxRowSize;
        float xPos = startPosition.x + col * spacingX;
        float yPos = startPosition.y - row * spacingYFields;

        buttonObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
    }
    private void OnFieldButtonClicked(string fieldName, string categoryPath) {
        string path = Path.Combine(categoryPath, fieldName);
        audioFiles = new List<string>(Directory.GetFiles(path, "*.wav"));
        ClearAudioContainer();
        int currentButtonIndex = 0;
        foreach (var filePath in audioFiles) {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            CreateFileButton(fileName, fieldName, categoryPath, currentButtonIndex);
            currentButtonIndex++;
        }
        CreateAddNewAudioButton(fieldName, currentButtonIndex);

    }

    private void OnTestAudioButtonClicked(string fieldName) {
        foreach (var field in currentGalleryCategory.GetAudioClipFields()) {
            if (field.Name == fieldName) {
                AudioClip clip = field.GetValue(currentGalleryCategory) as AudioClip;
                if (clip != null) {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
                else {
                    Debug.Log("Nothing set on this field");
                }
                break;
            }
        }
    }
    private void CreateFileButton(string fileName,string fieldName, string categoryPath, int index) {
        GameObject buttonObj = Instantiate(fileButtonPrefab, fileListContainer);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        Button deleteButton = buttonObj.transform.GetChild(1).GetComponent<Button>();
        buttonText.text = fileName;

        // Load the saved audio settings for the current field
        string savedAudioFileName = currentGalleryCategory.GetCurrentFieldSetting(fieldName);

        // Check if the current file name matches the saved file name
        if (savedAudioFileName == fileName + ".wav") { // Assuming the saved file name includes the .wav extension
            buttonObj.GetComponent<Image>().color = Color.blue;
        }
        else {
            buttonObj.GetComponent<Image>().color = Color.gray;
        }
        button.onClick.AddListener(() => OnFileButtonClicked(fileName,fieldName,categoryPath));
        deleteButton.onClick.AddListener(() => OnDeleteFileButtonClicked(fileName,fieldName,categoryPath));

        int row = index / maxRowSize;
        int col = index % maxRowSize;
        float xPos = startPosition.x + col * spacingX;
        float yPos = startPosition.y - row * spacingY;

        buttonObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
    }
   
    //
    private void OnFileButtonClicked(string fileName, string fieldName, string categoryPath) {
        string filePath = Path.Combine(categoryPath, fieldName);
        filePath = Path.Combine(filePath, fileName) + ".wav";
        AudioClip savedClip = NAudioPlayer.LoadWav(filePath);
        savedClip.name = fileName;
        if (savedClip != null) {
            foreach (var field in currentGalleryCategory.GetAudioClipFields()) {
                if (field.Name == fieldName) {
                    field.SetValue(currentGalleryCategory, savedClip);
                    currentGalleryCategory.SaveAudioSetting(field.Name, fileName + ".wav");
                    break;
                }
            }
        }
        else {
            UnityEngine.Debug.LogError("Failed to load saved audio clip.");
        }
        ClearAudioContainer();
        OnFieldButtonClicked(fieldName, categoryPath);
    }
    private void OnDeleteFileButtonClicked(string fileName, string fieldName, string categoryPath) {
        string fieldPath = Path.Combine(categoryPath, fieldName);
        string filePath = Path.Combine(fieldPath, fileName) + ".wav";

        if (File.Exists(filePath)) {
            try {
                // Delete the file
                File.Delete(filePath);

                // Update the save data by setting the filename to an empty string
                string savedAudioFileName = currentGalleryCategory.GetCurrentFieldSetting(fieldName);

                // Check if the current file name matches the saved file name
                if (savedAudioFileName == fileName + ".wav") { 
                    currentGalleryCategory.RemoveAudioSetting(fieldName);
                    foreach (var field in currentGalleryCategory.GetAudioClipFields()) {
                        if (field.Name == fieldName) {
                            field.SetValue(currentGalleryCategory, null);
                            SoundManager.Instance.audioGalleryEntries.InitializeAudioClips();
                            currentGalleryCategory.SaveAudioSetting(field.Name, fileName + ".wav");
                            break;
                        }
                    }
                }
                
                ClearAudioContainer();
                OnFieldButtonClicked(fieldName,categoryPath);
            }
            catch (Exception ex) {
                Debug.LogError("Error deleting file: " + ex.Message);
            }
        }
        else {
            Debug.LogWarning("File not found: " + filePath);
        }
    }


    private void CreateAddNewAudioButton(string fieldName, int index) {
        GameObject buttonObj = Instantiate(addAudioButtonPrefab, fileListContainer);
        Button button = buttonObj.GetComponent<Button>();

        button.onClick.AddListener(() => StartCoroutine(AddNewAudioFile(fieldName)));

        int row = index / maxRowSize;
        int col = index % maxRowSize;
        float xPos = startPosition.x + col * spacingX;
        float yPos = startPosition.y - row * spacingY;

        buttonObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
    }


    private IEnumerator AddNewAudioFile(string fieldName) {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select Audio File", "Load");

        if (FileBrowser.Success) {
            string filePath = FileBrowser.Result[0];
          
            byte[] fileData = File.ReadAllBytes(filePath);
            AudioClip audioClip = NAudioPlayer.FromMp3Data(fileData);
            if (audioClip != null) {
                string categoryPath = Path.Combine(savedAudioPath, currentGalleryCategory.name);
                string path = Path.Combine(categoryPath, fieldName, Path.GetFileNameWithoutExtension(filePath) + ".wav");
                NAudioPlayer.SaveWav(AudioClipToWav.Convert(audioClip), path);

                // Refresh the list
                ClearAudioContainer();
                OnFieldButtonClicked(fieldName,categoryPath);

            }
            else {
                UnityEngine.Debug.LogError("Failed to create audio clip from the provided file.");
            }
        }
    }

    public void ChooseRandomBGM() {
        // Path to the BGM category in the saved audio directory
        string bgmCategoryPath = Path.Combine(savedAudioPath,SoundManager.Instance.audioGalleryEntries.BGMCategory.name, "normalBgMusic");

        // Get all .wav files in the BGM category directory
        string[] bgmFiles = Directory.GetFiles(bgmCategoryPath, "*.wav");

        if (bgmFiles.Length == 0) {
            Debug.LogWarning("No BGM files found in the saved audio path.");
            return;
        }

        // Randomly select a BGM file
        int randomIndex = UnityEngine.Random.Range(0, bgmFiles.Length);
        string selectedBgmFilePath = bgmFiles[randomIndex];

        // Load the selected BGM as an AudioClip
        AudioClip randomBgmClip = NAudioPlayer.LoadWav(selectedBgmFilePath);

        if (randomBgmClip == null) {
            Debug.LogError("Failed to load the selected BGM file as an AudioClip.");
            return;
        }

        // Find the appropriate BGM field in the BGM category and set the clip
        var bgmCategory = SoundManager.Instance.audioGalleryEntries.BGMCategory;

        // Assuming there is a specific field in the BGM category where you want to set this clip
        foreach (var field in bgmCategory.GetAudioClipFields()) {
            if (field.Name == "normalBgMusic") {
                field.SetValue(bgmCategory, randomBgmClip);
                bgmCategory.SaveAudioSetting(field.Name, Path.GetFileName(selectedBgmFilePath));
                Debug.Log("Random BGM set: " + randomBgmClip.name);
                break;
            }
        }
    }


    private IEnumerator ImportAllFromFolder() {
        // Open a folder selection dialog
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Load");

        if (FileBrowser.Success) {
            string selectedFolderPath = FileBrowser.Result[0];

            // Enqueue files for processing
            foreach (string categoryFolderPath in Directory.GetDirectories(selectedFolderPath)) {
                string categoryName = Path.GetFileName(categoryFolderPath);

                // Check if this folder name matches any category in your GalleryCategory list
                foreach (var galleryCategory in SoundManager.Instance.audioGalleryEntries.GetCategories()) {
                    if (galleryCategory != null && galleryCategory.name == categoryName) {
                        foreach (string fieldFolderPath in Directory.GetDirectories(categoryFolderPath)) {
                            string fieldName = Path.GetFileName(fieldFolderPath);
                            foreach (var field in galleryCategory.GetAudioClipFields()) {
                                if (field.Name == fieldName) {
                                    foreach (string mp3FilePath in Directory.GetFiles(fieldFolderPath, "*.mp3")) {
                                        fileQueue.Enqueue(new FileProcessData(mp3FilePath, categoryName, fieldName));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Start the processing coroutine
            isProcessing = true;
            StartCoroutine(ProcessFiles());
        }
        ClearAudioContainer();
    }

    private IEnumerator ProcessFiles() {
        while (fileQueue.Count > 0) {
            FileProcessData fileData = fileQueue.Dequeue();

            // Process the file using the category and field information
            yield return StartCoroutine(ProcessFile(fileData));

            // Wait for memory to be managed after processing each file
            //yield return StartCoroutine(WaitForLowMemoryUsage());
        }

        isProcessing = false;
    }

    private IEnumerator ProcessFile(FileProcessData fileData) {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileData.Mp3FilePath);
        string savePath = Path.Combine(savedAudioPath, fileData.CategoryName, fileData.FieldName, fileNameWithoutExtension + ".wav");
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

        using (var mp3Reader = new Mp3FileReader(fileData.Mp3FilePath))
        using (var waveStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
        using (var waveFileWriter = new WaveFileWriter(savePath, waveStream.WaveFormat)) {
            waveStream.CopyTo(waveFileWriter);
        }

        // Force garbage collection and unload unused assets
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        // Yield to avoid blocking the main thread
        yield return null;
    }

    private IEnumerator WaitForLowMemoryUsage() {
        const long maxMemoryUsage = 1L * 1024L * 1024L * 1024L; // 1 GB
        while (System.GC.GetTotalMemory(true) > maxMemoryUsage) {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            yield return new WaitForSeconds(1f); // Wait a bit longer to allow memory to free up
        }
    }

    // Data structure to hold file information
    private class FileProcessData {
        public string Mp3FilePath { get; }
        public string CategoryName { get; }
        public string FieldName { get; }

        public FileProcessData(string mp3FilePath, string categoryName, string fieldName) {
            Mp3FilePath = mp3FilePath;
            CategoryName = categoryName;
            FieldName = fieldName;
        }
    }
    // Function to handle reset button popup
    private void OnResetDefaultsClicked() {
        PopupManager.Instance.ShowConfirmationPopup(ResetAllFieldsToDefault);
    }
    // Function to reset all fields to their default values
    private void ResetAllFieldsToDefault() {
        foreach (var galleryCategory in SoundManager.Instance.audioGalleryEntries.GetCategories()) {
            if (galleryCategory != null) {
                foreach (var field in galleryCategory.GetAudioClipFields()) {
                    // Set each field to its default value (null in this case)
                    field.SetValue(galleryCategory, null);

                    // Remove any saved settings for this field
                    galleryCategory.RemoveAudioSetting(field.Name);
                    galleryCategory.LoadAudioSettings();
                }
            }
        }
        // Optionally, clear the UI to reflect these changes
        ClearAudioContainer();
    }
    private void ResetFieldToDefault(string fieldName) {

        foreach (var field in currentGalleryCategory.GetAudioClipFields()) {
            if(field.Name == fieldName) {
                // Set each field to its default value (null in this case)
                field.SetValue(currentGalleryCategory, null);

                // Remove any saved settings for this field
                currentGalleryCategory.RemoveAudioSetting(field.Name);
                currentGalleryCategory.LoadAudioSettings();

            }
        }

        // Optionally, clear the UI to reflect these changes
        ClearAudioContainer();
        DisplayCategoryFields(currentGalleryCategory);
    }
}
