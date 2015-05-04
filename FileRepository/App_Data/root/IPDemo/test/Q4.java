
/*
 * Check if a given array contains duplicate elements within k distance from each other
 * Given an unsorted array that may contain duplicates. Also given a number k which is smaller
 *  than size of array. Write a function that returns true if array contains duplicates within 
 *  k distance.
 */

package arrayGeeks;

import java.util.HashSet;

public class Q4 {

	public static boolean returnDupExist(int[] arr, int k)
	{
		if(k > arr.length) return false;
		
		HashSet<Integer> H1 = new HashSet<Integer>();
		
		for(int i = 0 ; i < arr.length ; i++)
			{
			  if(H1.contains(arr[i])) 
				  return true;
			  else H1.add(arr[i]);
			
			  if(i>=k)
			  H1.remove(arr[i-k]) ;
			}
		
		return false ;
	}
	
	public static void main(String[] args) {
		
		int arr1[]= {1, 2, 3, 4, 1, 2, 3, 4};
		int k = 3 ;
		System.out.println("arr1 contains dups within " + k + " elements :" + returnDupExist(arr1,k));
		
		int arr2[]= {1, 2, 3, 1, 4, 5};
		k = 3 ;
		System.out.println("arr2 contains dups within " + k + " elements :" + returnDupExist(arr2,k));
		
		int arr3[]= {1, 2, 3, 4, 5};
		k = 3 ;
		System.out.println("arr3 contains dups within " + k + " elements :" + returnDupExist(arr3,k));
		
		int arr4[]= {1, 2, 3, 4, 4};
		k = 3 ;
		System.out.println("arr4 contains dups within " + k + " elements :" + returnDupExist(arr4,k));

	}

}
