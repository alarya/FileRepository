/*
 * Find the first repeating element in an array of integers
 * Given an array of integers, find the first repeating element in it. 
 * We need to find the element that occurs more than once and whose index of
 *  first occurrence is smallest.
 */

package arrayGeeks;

import java.util.HashMap;

public class Q6 {

	
	public static int returnIndex(int arr[]){
		int Index = Integer.MAX_VALUE ;
		
		if(arr.length == 0) return Index ;
		
		HashMap<Integer, Integer> H1 = new HashMap<Integer, Integer>();
		
		for(int i = 0 ; i < arr.length; i++)
		{
			if(H1.containsKey(arr[i]))
			{
				if(H1.get(arr[i]) < Index)
					Index = H1.get(arr[i]) ;
			}
			else
			H1.put(arr[i], i) ;
								
		}		
		return arr[Index];		
 	}
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub

		int arr1[] = {10, 5, 3, 4, 3, 5, 6};
		System.out.println("Result: " + returnIndex(arr1));
		
		int arr2[]= {6, 10, 5, 4, 9, 120, 4, 6, 10};
		System.out.println("Result: " + returnIndex(arr2));
		
		int arr3[] = {1,2,3,4,4,3,1,0};
		System.out.println("Result: " + returnIndex(arr3));
	}

}
