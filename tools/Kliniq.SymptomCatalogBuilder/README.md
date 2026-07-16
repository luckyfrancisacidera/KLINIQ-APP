# KLINIQ Symptom Catalog Builder

This is an offline, deterministic .NET 10 console tool. It never runs inside the API and makes no network calls. It converts manually downloaded public disease/symptom exports into a reviewable specialty phrase catalog.

## Refresh workflow

1. Review each dataset's current license and download/export it manually.
2. Place the files in `tools/Kliniq.SymptomCatalogBuilder/data/` using the names in `data/README.md`.
3. Download the latest HPO JSON release as `data/hp.json` from the official HPO release/download page.
4. Update `data/icd10-specialty-map.json` only after a disease-to-specialty mapping has been reviewed. Uncertain diseases must remain unmapped.
5. Run from the repository root:

```bash
dotnet run --project tools/Kliniq.SymptomCatalogBuilder
```

The command fails before writing output when no local dataset export is present. This prevents an accidental baseline-only overwrite. `hp.json` is optional, but at least one dataset export is required.

6. Review all three outputs before committing:
   - `server/Kliniq/src/Kliniq.Application/Resources/symptom-catalog.json`
   - `tools/Kliniq.SymptomCatalogBuilder/catalog-diff.md`
   - `tools/Kliniq.SymptomCatalogBuilder/data/unmapped-diseases.txt`
7. Remove the downloaded raw dataset exports before committing unless their licenses and repository policy explicitly permit redistribution.

The builder normalizes case/spacing/punctuation, splits multi-symptom fields, filters generic unsafe phrases, maps diseases through the checked-in ICD-10-oriented lookup, expands exact HPO synonyms when `hp.json` is present, and deduplicates output. It never guesses a specialty.

## Sources and licenses

- Symptom2Disease: Kaggle `niyarrbarman/symptom2disease`; CC0 shown on Kaggle.
- Gretel symptom-to-diagnosis: Hugging Face `gretelai/symptom_to_diagnosis`; Apache-2.0.
- Disease Symptom Prediction: Kaggle `itachi9604/disease-symptom-description-dataset`; CC BY-SA 4.0 shown on Kaggle.
- Disease-Symptom Dataset: Kaggle `dhivyeshrk/diseases-and-symptoms-dataset`; World Bank Dataset Terms of Use shown on Kaggle.
- Diseases_Symptoms: Hugging Face `QuyenAnhDE/Diseases_Symptoms`; no license declaration was visible when this tool was authored, so do not redistribute its raw or derived data without clarification.
- HPO: official Human Phenotype Ontology release; follow the HPO license and citation instructions.

## Known non-goal

Filipino and Ilocano phrases are not generated from these English datasets. Add them only through a separately reviewed, clinician-curated translation process.

## Runtime separation

The console project is not referenced by the API and is never executed at application startup. The API reads only the manually reviewed embedded `symptom-catalog.json`. Downloaded CSV/JSON exports and `hp.json` are ignored by Git.

## Review policy

A generated phrase is eligible only when its disease has an explicit reviewed entry in `icd10-specialty-map.json`. Chapter metadata documents the clinical grouping, but the builder does not infer a specialty from an unfamiliar disease name. Add mappings deliberately, rerun, inspect `catalog-diff.md`, and leave uncertain diseases in `unmapped-diseases.txt`.
