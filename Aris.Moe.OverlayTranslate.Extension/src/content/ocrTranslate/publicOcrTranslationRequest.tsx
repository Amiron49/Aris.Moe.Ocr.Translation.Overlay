export class PublicOcrTranslationRequest {
    imageUrl!: string;
    imageHash?: number[] | null;
    height?: number;
    width?: number;
    ApiKey?: string;
}