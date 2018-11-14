import { Injectable } from '@angular/core';

import { ClaimGuardBase } from '../shared/services/models';

@Injectable()
export class AdministratorClaimGuard extends ClaimGuardBase {
  public static CLAIM_NAME = 'workflow_admin';
  protected claim = AdministratorClaimGuard.CLAIM_NAME;
}
