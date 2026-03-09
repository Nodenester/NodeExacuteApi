# NodeExacuteApi

**Block execution engine for [NodeNestor/AiDesigner](https://github.com/NodeNestor)**

Built September 2023 -- February 2024, 51 commits.

## What it does

REST API that executes node-based visual programs. Loads program graphs from a database, resolves block connections, executes blocks in dependency order, and returns results. This is the backend engine that powers AiDesigner.

## Block types

- **LLMs** -- Llama 2 (7B, 13B, 70B), Zephyr 7B, Mixtral 8x7B, CodeLlama 34B
- **Image AI** -- Stable Diffusion XL, DALL-E 3 XL, image classification, segmentation, captioning, object detection/cropping, OCR
- **Audio AI** -- Whisper transcription, text-to-speech, music generation, speech enhancement
- **Summarization** -- BART, Flan-T5, factual consistency checking
- **OpenAI** -- GPT-3.5/GPT-4 integration
- **Math** -- arithmetic, rounding, trigonometry, random, min/max, absolute value
- **String** -- concatenation, split, replace, substring, case conversion, regex, length, contains, trim
- **Boolean** -- AND, OR, NOT, XOR
- **Comparison** -- equal, not equal, greater/less than, between, null check
- **List** -- add, remove, get, set, count, sort, filter, contains, reverse, flatten, zip, distinct
- **Image manipulation** -- resize, crop, rotate, flip, overlay, brightness, contrast, grayscale, blur, color filter
- **Sound** -- volume, trim, concatenate, reverse, speed, fade, mix
- **Control flow** -- if/else, switch, for loop, while loop, index loop

## Token billing

Each AI model call deducts tokens from the user's account based on model-specific pricing multipliers.

## Tech stack

- C# / ASP.NET Core
- SQL Server with Dapper
- HuggingFace Inference API
- OpenAI API
- TOTP-based test authentication

## Status

Archived -- no longer maintained.

## License

MIT
