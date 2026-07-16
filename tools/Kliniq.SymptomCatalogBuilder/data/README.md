# Local dataset exports

This directory intentionally does **not** redistribute third-party datasets. Download/export each source yourself, confirm its current license, and place it here with one of the filenames below:

| Source | Expected local filename |
|---|---|
| Kaggle `niyarrbarman/symptom2disease` | `symptom2disease.csv` |
| Hugging Face `gretelai/symptom_to_diagnosis` | `gretel-symptom-to-diagnosis.jsonl`, `.json`, or `.csv` |
| Kaggle `itachi9604/disease-symptom-description-dataset` | `disease-symptom-description.csv` |
| Kaggle `dhivyeshrk/diseases-and-symptoms-dataset` | `diseases-and-symptoms.csv` |
| Hugging Face `QuyenAnhDE/Diseases_Symptoms` | `quyenanh-diseases-symptoms.json` or `.csv` |
| Human Phenotype Ontology | `hp.json` |

`baseline-hardcoded-catalog.json`, `icd10-specialty-map.json`, and `generic-symptom-stoplist.json` are maintained by KLINIQ and are checked in. The builder writes `unmapped-diseases.txt` here.

The files listed as local exports, including `hp.json`, are ignored by Git. The builder will fail safely if none of the five dataset exports is present. HPO expansion is skipped with a warning when `hp.json` is absent.
