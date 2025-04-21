using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// ����Ի�������������峤��������ĶԻ�
/// </summary>
public class FirstSceneDialogueManager : DialogueDisplayBase
{
    // ��ǰ������ʾ�ľ���ID
    private string currentSentenceId;

    private RectTransform evidencePanel;

    // �̶����¼�ID
    public override string EventId => "FirstScene";


    /// <summary>
    /// ���캯��
    /// </summary>
    public FirstSceneDialogueManager(
        MonoBehaviour runner,
        TextMeshProUGUI dialogueText,
        TextMeshProUGUI speakerNameText,
        UnityEngine.UI.Image characterImage,
        GameObject dialogueContainer,
        RectTransform evidencePanel = null)
        : base(runner, dialogueText, speakerNameText, characterImage, dialogueContainer)
    {
        this.evidencePanel = evidencePanel;
    }

    

    /// <summary>
    /// �ƽ��Ի�
    /// </summary>
    public override void AdvanceDialogue()
    {
        // ���������ʾ�ı�����������ɵ�ǰ������ʾ
        if (IsDisplayingText())
        {
            CompleteCurrentSentence();
            return;
        }

        // ʹ��switch-case�ṹ���ƶԻ�����
        switch (dialogueState)
        {
            case 0:
                BackgroundManager.Instance.SwitchToBackground(1);
                currentSentenceId = "sentence_001";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 1:
                currentSentenceId = "sentence_002";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 2:
                currentSentenceId = "sentence_003";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 3:
                currentSentenceId = "sentence_004";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 4:
                currentSentenceId = "sentence_005";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 5:
                currentSentenceId = "sentence_006";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 6:
                currentSentenceId = "sentence_007";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 7:
                currentSentenceId = "sentence_008";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 8:
                currentSentenceId = "sentence_009";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 9:
                currentSentenceId = "sentence_010";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 10:
                currentSentenceId = "sentence_011";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 11:
                currentSentenceId = "sentence_012";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 12:
                currentSentenceId = "sentence_013";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 13:
                currentSentenceId = "sentence_014";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 14:
                currentSentenceId = "sentence_015";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 15:
                currentSentenceId = "sentence_016";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState = 27;
                DialogueController.Instance.SetInputEnabled(false);
                StartCoroutine(WaitAndAdvance());
                break;

            case 16:
                currentSentenceId = "sentence_018";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState = 18;
                break;

            case 17:
                if (IsDisplayingText())
                {
                    CompleteCurrentSentence();
                    break;
                }

                if (currentClickedEvidenceId != null)
                {
                    if (currentClickedEvidenceId == "evidence_package")
                    {
                        dialogueState = 16;
                        CleanupEvidencesAndRestoreInput();
                        AdvanceDialogue();
                    }
                    else
                    {
                        currentSentenceId = currentClickedEvidenceId;
                        typingCoroutine = StartCoroutine(ShowSentenceAndRepeat(currentSentenceId));
                    }

                    currentClickedEvidenceId = null;
                }
                break;

            case 18:
                currentSentenceId = "sentence_019";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;

            case 19:
                currentSentenceId = "sentence_020";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 20:
                currentSentenceId = "sentence_021";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 21:
                currentSentenceId = "sentence_022";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 22:
                currentSentenceId = "sentence_023";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 23:
                currentSentenceId = "sentence_017";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 24:
                currentSentenceId = "sentence_024";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 25:
                currentSentenceId = "sentence_025";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                dialogueState++;
                break;
            case 27:
                currentSentenceId = "empty";
                typingCoroutine = StartCoroutine(DisplaySentence(currentSentenceId));
                PlaceEvidences();
                dialogueState = 17;
                break;

            default:
                Debug.LogWarning("δ����ĶԻ�״̬: " + dialogueState);
                EndDialogue();
                break;
        }
    }

    private void PlaceEvidences()
    {
        // ���û��ָ����壬��ʹ�öԻ�����
        Transform parentTransform = evidencePanel != null ?
        (Transform)evidencePanel : dialogueContainer.transform;

        // ����֤����Ϣ�б�
        List<(string id, string name, string description, Vector2 position, Vector2 size)> evidenceInfos =
            new List<(string, string, string, Vector2, Vector2)>
        {
        ("evidence_plant", "��ֲ", "��ֲ", new Vector2(-238.2f, 31.1f), new Vector2(367.73f, 761.38f)),
        ("evidence_sign1", "��ʾ��1", "��ʾ��2", new Vector2(77.64f, 362.4f), new Vector2(155.3f, 234.73f)),
        ("evidence_sign2", "��ʾ��2", "��ʾ��2", new Vector2(360.43f, 381.57f), new Vector2(212.12f, 291.58f)),
        ("evidence_bell", "����", "����", new Vector2(-910.48f, 130f), new Vector2(99.05f, 101.88f)),
        ("evidence_shelf", "�����Ƽ���", "�����Ƽ���", new Vector2(-765.66f, 385.23f), new Vector2(388.67f, 309.54f)),
        ("evidence_package", "ɳ���ϵİ�", "ɳ���ϵİ�", new Vector2(543f, -261f), new Vector2(368.731f, 363.4f))


        };

        // ����֤��
        foreach (var info in evidenceInfos)
        {
            EvidenceManager.Instance.CreateEvidence(
                info.id, info.name, info.description,
                parentTransform, info.position, info.size);
        }

        // ��ʼ����֤����
        StartListeningForEvidenceClicks();

        Debug.Log("�ѷ���֤�ﲢ���ó���Ի��ƽ�");
    }

    /// <summary>
    /// ����֤�ﲢ�ָ��Ի��ƽ�
    /// </summary>
    private void CleanupEvidencesAndRestoreInput()
    {
        // ֹͣ����֤����
        StopListeningForEvidenceClicks();

        // ��������֤��
        EvidenceManager.Instance.DestroyAllEvidence();

        // �������ó���Ի��ƽ�
        DialogueController.Instance.SetInputEnabled(true);

        Debug.Log("������֤�ﲢ�������ó���Ի��ƽ�");
    }

    private IEnumerator WaitAndAdvance()
    {
        yield return StartCoroutine(DisplaySentence(currentSentenceId));
        yield return new WaitForSeconds(0.5f); // �ȴ� 0.5 �����ƽ�
        AdvanceDialogue();
    }

    private IEnumerator ShowSentenceAndRepeat(string sentenceId)
    {
        // ��ʾ�������ľ���
        yield return StartCoroutine(DisplaySentence(sentenceId));
        // ��΢��һ�£���ֹ���������
        yield return new WaitForSeconds(0.5f);

        // ���ضԻ������������ڵ�֤��
        if (!IsDisplayingText())
        {
            dialogueContainer.SetActive(false);
        }
        
        // �ٴδ��� Advance���ط� case 17
        AdvanceDialogue();
    }
}