import React from 'react'
import PropTypes from 'prop-types'
import Information from './Information'
import Analytics from './Analytics'
import { Tabs, Tab } from 'components/PanelTabs'

export const SideBar = (props) => (
  <Tabs>
    <Tab title='Information'>
      <Information {...props} />
    </Tab>
    <Tab title='Analytics'>
      <Analytics />
    </Tab>
  </Tabs>
)

SideBar.propTypes = {
  node: PropTypes.object.isRequired,
  type: PropTypes.object.isRequired,
  data: PropTypes.object,
  onDelete: PropTypes.func.isRequired,
  onRename: PropTypes.func.isRequired,
  onChangeType: PropTypes.func.isRequired,
  onMove: PropTypes.func.isRequired
}

export default SideBar
