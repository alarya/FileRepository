/*
 * Given an array A[] and a number x, check for pair in A[] with sum as x
 * 
 */


package arrayGeeks;

import java.util.Arrays;
import java.util.HashMap;

public class Q1 {

	//Method 1 - using sorting - O(nLogn)
	public static boolean sumPair1(int[] arr, int x)
	{
		Arrays.sort(arr);  //assuming its an nlogn sorting algo like merge or quick
		int i = 0 ;
		int j = arr.length - 1;
		
		while(i<j)
		{
			if(arr[i] + arr[j] == x)
				return true;
			else if(arr[i] + arr[j] > x) j--;
			else i++ ;
		}
		return false;
	}
	
	//Method 2 - using HashMap - runs in linear time
	public static boolean sumPair2(int[] arr, int x)
	{
		HashMap<Integer,String> H1 = new HashMap<Integer,String>();
		
		for(int i = 0 ; i < arr.length ; i ++)
		{
			if(H1.containsKey(arr[i]))
				return true;
			else
				H1.put((x - arr[i]), "true");
		}
		return false ;
	}
	
	public static void main(String[] args) {
		
		//test cases;
		int[] arr1 = {1, 4, 45, 6, 10, -8};
		int x = 16 ;
		System.out.println("pair exist: " + sumPair1(arr1, x));
		System.out.println("pair exist: " + sumPair2(arr1, x));
		
		int[] arr2 = {1,2,3} ;
		System.out.println("pair exist: " + sumPair1(arr2, x));
		System.out.println("pair exist: " + sumPair2(arr2, x));
		
		int[] arr3 = {3,15,1} ;
		System.out.println("pair exist: " + sumPair1(arr3, x));
		System.out.println("pair exist: " + sumPair2(arr3, x));
	}

}
