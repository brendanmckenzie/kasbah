import { connect } from 'react-redux'
import { actions as contentActions } from 'store/appReducers/content'

import View from './components/View'

const mapDispatchToProps = {
  ...contentActions
}

const mapStateToProps = (state) => ({
  content: state.content
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
