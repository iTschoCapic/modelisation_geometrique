# Geometric Modelling

This document summarises the various tutorials (1 to 6) on algorithms and implementations related to geometry and mesh manipulation in Unity.

---

## TD 1: Introduction to meshes**

**Objective:** Understand the basics of mesh manipulation in Unity.

- **Creating simple shapes:**
  Implement a script to create a mesh from points (vertices) and triangles.
  
- **Key points:**
  - Definition of vertices (using `Vector3`).
  - Definition of triangles (using an array of indices).
  - Apply the mesh to a `MeshFilter` to make it visible.

![TD1_screenshot](./screenshots/TD1.png)

---

## **TD 2: Calculating Normals**

**Objective:** Implement the calculation of normals for meshes in order to improve their rendering.

- **Key points:**
  - Calculate normals from vertices and triangles.
  - Normalisation of normal vectors to guarantee their unity.
  - Comparison of rendering with and without calculating normals.

![TD2_screenshot](./screenshots/TD2.png)

---

## **TD 3: Mesh division and deformation**

**Objective:** Implement an adaptive octree algorithm for the 3D volumetric representation of spheres using spatial enumeration.

- **Exercise:** Determine the minimum bounding box for each sphere based on its **centre** and **radius**, optimising collision detection and spatial queries.
- **Key points:**
  - Iterate over a set of spheres to generate individual representations.  
  - Approximate the volume of each sphere with cubes, calculating intersections between the cubes and the sphere for precise detail.
  - Split cubes that partially intersect spheres into smaller cubes.
  - Check intersections with spheres at each recursion level to improve accuracy without redundancy.

![TD3_screenshot](./screenshots/TD3.png)

---

## **TD 4: Simplification by Partitioning**

**Objective:** Reduce the number of vertices in a mesh by merging nearby vertices.

- **Key points:**
  - Construct a grid to group nearby vertices.
  - Merge the vertices in each cell by calculating their average.
  - Update the triangles with the new indices of the merged vertices.

- **Parameters:**
  - Cell size (controlling the resolution of the resulting mesh).

---

## **TD 5 : Subdivision of Curves**

### **Exercise 1: Chaikin algorithm**

**Objective:** Implement the Chaikin algorithm to smooth a curve.

- **Key points:**
  - At each iteration, divide each segment of the curve into two points calculated at 25% and 75% between the ends.
  - Generates a smooth curve from a coarse polygon.

---

## **TD 6: Curves and Advanced Algorithms**

### **Exercise 1: Hermite curves**

**Objective:** Draw Hermite curves defined by two points and their tangents.

- **Key points:**
  - Use Hermite coefficients to interpolate a curve.
  - Interaction possible to adjust tangents in real time.

### **Exercise 2: Cubic Bézier curves**

**Objective:** Implement Bézier curves using 4 control points.

- **Key points:**
  - Parametric calculation using control points.
  - Visualisation of control polygons and curve segments.

### **Exercise 3 (Option) : Casteljau algorithm**

**Objective:** Generate Bézier curves from any number of control points.

- **Key points:**
  - Recursive implementation of the Casteljau algorithm.
  - Allows you to manipulate complex curves with dynamic interactions.

Screenshot in order of Hermite, Bézier and Casteljau curves
![TD6_screenshot](./screenshots/TD6.png)

---

## **Conclusion**

Thanks to Corentin Le Bihan Gautier for these tutorials, which provide an introduction to manipulating meshes and curves in Unity, covering concepts such as subdivision, simplification and parametric curves.

**Suggestions:**
- Add animations to visualise processes in real time.
- Export the generated curves and meshes for use in other projects.

**Next step:** Deepen the shaders to make the meshes even more realistic.
