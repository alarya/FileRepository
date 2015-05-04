
/*
 * Find the closest pair from two sorted arrays
 * Given two sorted arrays and a number x, find the pair whose sum is closest to x and
 * the pair has an element from each array.
 */

package arrayGeeks;

class ArrayPair{
	public int x;
	public int y;
}

public class Q7 {

	public static ArrayPair returnPair1(int[] arr1, int[] arr2, int x){
		
	   if(arr1.length == 0 || arr2.length == 0)
		   return null;
	   
	   int i = 0;
	   int j = arr2.length - 1;
	   int difference =  Integer.MAX_VALUE;
	   ArrayPair result = new ArrayPair();
	   
	   while(i <= j )
	   {
		   if(Math.abs(arr1[i] + arr2[j] - x) < difference)
		   {
			   difference = Math.abs(arr1[i] + arr2[j] - x) ;
			   result.x = arr1[i];
			   result.y = arr2[j];
		   }
		   
		   if(arr1[i] + arr2[j] > x)
			   j--;
		   else
			   i++;
	   }	   
	   return result;
	}
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		
		int[] arr1 = {1, 4, 5, 7};
		int[] arr2 = {10, 20, 30, 40};
		int x = 32;
		ArrayPair result = returnPair1(arr1, arr2, x);
		System.out.println( result.x + " " + result.y);
		
		x = 50;
		result = returnPair1(arr1, arr2, x);
		System.out.println( result.x + " " + result.y);
		
		x = 2;
		result = returnPair1(arr1, arr2, x);
		System.out.println( result.x + " " + result.y);
		

	}

}
