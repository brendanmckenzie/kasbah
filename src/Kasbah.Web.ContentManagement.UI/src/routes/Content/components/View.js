import React from 'react'
import PropTypes from 'prop-types'
import ContentTree from './ContentTree/index.js'

export const View = (props) => (
  <div className='section'>
    <div className='container'>
      <h1 className='title'>Content</h1>
      <div className='columns'>
        <div className='column is-2'>
          <ContentTree
            {...props} />
        </div>
        <div className='column'>
          {props.children}
        </div>
      </div>
    </div>
  </div>
)

View.propTypes = {
  children: PropTypes.node,
  describeTree: PropTypes.object.isRequired,
  describeTreeRequest: PropTypes.func.isRequired,
  createNode: PropTypes.object.isRequired,
  createNodeRequest: PropTypes.func.isRequired,
  listTypes: PropTypes.object.isRequired,
  listTypesRequest: PropTypes.func.isRequired,
  deleteNode: PropTypes.object.isRequired,
  updateNodeAlias: PropTypes.object.isRequired,
  changeType: PropTypes.object.isRequired,
  moveNode: PropTypes.object.isRequired
}

export default View
