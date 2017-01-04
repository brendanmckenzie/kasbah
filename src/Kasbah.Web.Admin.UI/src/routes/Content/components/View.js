import React from 'react'
import ContentTree from './ContentTree/index.js'

export const View = ({ children, describeTree, describeTreeRequest, createNode, createNodeRequest }) => (
  <div className='section'>
    <h1 className='title'>Content</h1>
    <div className='columns'>
      <div className='column is-2'>
        <ContentTree
          describeTree={describeTree}
          describeTreeRequest={describeTreeRequest}
          createNode={createNode}
          createNodeRequest={createNodeRequest} />
      </div>
      <div className='column'>
        {children}
      </div>
    </div>
  </div>
)

View.propTypes = {
  children: React.PropTypes.node,
  describeTree: React.PropTypes.object.isRequired,
  describeTreeRequest: React.PropTypes.func.isRequired,
  createNode: React.PropTypes.object.isRequired,
  createNodeRequest: React.PropTypes.func.isRequired
}

export default View
