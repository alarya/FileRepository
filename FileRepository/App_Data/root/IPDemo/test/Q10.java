
/*
 * Find if there is a subarray with 0 sum
Given an array of positive and negative numbers, find if there is a subarray with 0 sum.
 */


package arrayGeeks;

import java.util.HashSet;

public class Q10 {

	public static boolean subArray0(int[] arr){
		
		HashSet<Integer> H1 = new HashSet<Integer>();
		
		int sum = 0 ;
		for(int i = 0; i < arr.length ; i++){
			
			sum += arr[i];
			if(H1.contains(sum))
				return true;
			else
				H1.add(sum);
		}
		
		return false ;
	}
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub

		int[] arr1 = {4, 2, -3, 1, 6};
		System.out.println("Result: " + subArray0(arr1));
		
		int[] arr2 = {4, 2, 0, 1, 6};
		System.out.println("Result: " + subArray0(arr2));
		
		int[] arr3 = {-3, 2, 3, 1, 6};
		System.out.println("Result: " + subArray0(arr3));
	}

}
