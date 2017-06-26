import React from 'react'
import PropTypes from 'prop-types'
import Information from './Information'

export const SideBar = (props) => (
  <Information {...props} />
)

SideBar.propTypes = {
  node: PropTypes.object.isRequired,
  type: PropTypes.object.isRequired,
  data: PropTypes.object,
  onDelete: PropTypes.func.isRequired,
  onEdit: PropTypes.func.isRequired,
  onMove: PropTypes.func.isRequired
}

export default SideBar
