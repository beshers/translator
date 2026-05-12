using System.Threading;
using System.Threading.Tasks;
using translator.Models;

namespace translator.Services;

public interface ITranslationHistoryService
{
    Task SaveAsync(
        string sourceText,
        string translatedText,
        Language sourceLanguage,
        Language targetLanguage,
        CancellationToken cancellationToken = default);
}
