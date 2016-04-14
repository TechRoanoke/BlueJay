using System.Threading.Tasks;

namespace BlueJay
{
    public interface IBlueJayClient
    {
        // Get tags I created. Scoped to my token. 
        Task<UserTagItem[]> GetMyTagListAsync();

        // Get suggested tags. 
        Task<TagItem[]> GetSuggestedTagListAsync(string state);

        // Search for voters with the given query. 
        Task<Segment<VoterResult>> GetExtendedVoterLookupAsync(ExtendedLookupParameters query, string continuationToken = null);

        // Create a new VoterContact. This lets us save multiple responses. 
        Task<VoterContactResponse> PostVoterContactAsync(VoterContactRequest voterContact);

        // Post an 
        Task PostVoterContactDetailsAsync(params VoterContactDetailRequest[] details);
    }
}