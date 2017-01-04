import { connect } from 'react-redux'
import { action as getDetailRequest } from '../modules/getDetail'
import { action as putDetailRequest } from '../modules/putDetail'
import { action as publishRequest } from '../modules/publish'

import View from '../components/View'

const mapDispatchToProps = {
  getDetailRequest,
  putDetailRequest,
  publishRequest
}

const mapStateToProps = (state, ownProps) => ({
  id: ownProps.params.id,
  getDetail: state.getDetail,
  putDetail: state.putDetail,
  publish: state.publish
})

export default connect(mapStateToProps, mapDispatchToProps)(View)
