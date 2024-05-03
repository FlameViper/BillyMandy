using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePageList : MonoBehaviour
{
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private List<GameObject> pages = new List<GameObject>();
    private int currentPageIndex = 0;

    private void Awake() {
        // Hide all pages except the first one
        ShowPage(currentPageIndex);

        // Add listeners to the button click events
        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PrevPage);
    }

    private void NextPage() {
        // Hide the current page
        HidePage(currentPageIndex);

        // Increment the page index
        currentPageIndex++;

        // Wrap around if index exceeds the number of pages
        if (currentPageIndex >= pages.Count) {
            currentPageIndex = 0;
        }

        // Show the next page
        ShowPage(currentPageIndex);
    }

    private void PrevPage() {
        // Hide the current page
        HidePage(currentPageIndex);

        // Decrement the page index
        currentPageIndex--;

        // Wrap around if index goes below zero
        if (currentPageIndex < 0) {
            currentPageIndex = pages.Count - 1;
        }

        // Show the previous page
        ShowPage(currentPageIndex);
    }

    private void ShowPage(int index) {
        if (index >= 0 && index < pages.Count) {
            pages[index].SetActive(true);
        }
    }

    private void HidePage(int index) {
        if (index >= 0 && index < pages.Count) {
            pages[index].SetActive(false);
        }
    }
}
