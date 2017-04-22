import React from 'react'
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
  node: React.PropTypes.object.isRequired,
  type: React.PropTypes.object.isRequired,
  data: React.PropTypes.object,
  onDelete: React.PropTypes.func.isRequired,
  onRename: React.PropTypes.func.isRequired,
  onChangeType: React.PropTypes.func.isRequired,
  onMove: React.PropTypes.func.isRequired
}

export default SideBar
