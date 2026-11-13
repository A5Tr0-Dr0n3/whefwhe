using System.Collections.Generic;

public class JobReviewUtility
{
	private const int numConstantReviews = 2;

	public static int JobStateToReviewIndex(VVR_HUB.JobStates stateIn)
	{
		return (int)(stateIn + 2);
	}

	public static void PushJobReview(VVR_HUB.JobStates jobState, JobReview reviewIn)
	{
		PushJobReview((int)(jobState + 2), reviewIn);
	}

	public static void PushJobReview(int reviewIndex, JobReview reviewIn)
	{
		SerializableDictionary<int, JobReview> jobReviews = PersistentDataManager.GetJobReviews();
		if (jobReviews.ContainsKey(reviewIndex))
		{
			jobReviews[reviewIndex] = reviewIn;
		}
		else
		{
			jobReviews.Add(reviewIndex, reviewIn);
		}
		PersistentDataManager.OverwriteJobReviews(jobReviews);
	}

	public static JobReview[] GetJobReviews(JobReviewScriptableObject[] defaultReviews, int numRequested, JobReviewScriptableObject[] butterReviews)
	{
		SerializableDictionary<int, JobReview> jobReviews = PersistentDataManager.GetJobReviews();
		JobReview[] array = new JobReview[numRequested];
		List<JobReviewScriptableObject> list = new List<JobReviewScriptableObject>(butterReviews);
		for (int i = 0; i < numRequested; i++)
		{
			if (i < defaultReviews.Length)
			{
				if (jobReviews.ContainsKey(i))
				{
					array[i] = jobReviews[i];
				}
				else
				{
					array[i] = defaultReviews[i].mReview;
				}
			}
			else
			{
				array[i] = list[i % list.Count].mReview;
			}
		}
		while (array.Length < numRequested)
		{
		}
		return array;
	}
}
