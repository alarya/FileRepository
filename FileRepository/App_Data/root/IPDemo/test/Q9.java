/*
 * Find the number of zeroes
   Given an array of 1s and 0s which has all 1s first followed by all 0s. 
   Find the number of 0s. Count the number of zeroes in the given array.
 */

package arrayGeeks;

public class Q9 {

	public static int return0s(int[] arr, int low, int high){
		
		int mid = (low + high) / 2 ;
		
		if(mid == 0 && arr[mid] == 0)     //all zeros
			return arr.length;
		if(mid == arr.length - 1 && arr[mid] == 1) // all 1s
			return 0;
		
		if(arr[mid] == 1 && arr[mid+1] == 0)
			return arr.length - mid - 1;
		if(arr[mid] == 0 && arr[mid-1] == 1)
		    return arr.length - mid;

		
		if(arr[mid] == 1)
			return return0s(arr,mid+1,high);
		else 
			return return0s(arr,low,mid-1);
			    
	}
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub

		int[] arr = {1,1,1,1,0,0};
		System.out.println("No of zeros: " + return0s(arr, 0, arr.length -1));
		
		int[] arr1 = {1, 0, 0, 0, 0};
		System.out.println("No of zeros: " + return0s(arr1, 0, arr1.length -1));
		
		int[] arr2 = {0,0,0};
		System.out.println("No of zeros: " + return0s(arr2, 0, arr2.length -1));
		
		int[] arr3 = {1,1,1};
		System.out.println("No of zeros: " + return0s(arr3, 0, arr3.length -1));
	}

}
